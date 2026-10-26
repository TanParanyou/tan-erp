//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/opportunity_work_image_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'attach_work_images_response.g.dart';

/// AttachWorkImagesResponse
///
/// Properties:
/// * [items] 
/// * [opportunityRowVersion] 
@BuiltValue()
abstract class AttachWorkImagesResponse implements Built<AttachWorkImagesResponse, AttachWorkImagesResponseBuilder> {
  @BuiltValueField(wireName: r'items')
  BuiltList<OpportunityWorkImageResponse>? get items;

  @BuiltValueField(wireName: r'opportunityRowVersion')
  String? get opportunityRowVersion;

  AttachWorkImagesResponse._();

  factory AttachWorkImagesResponse([void updates(AttachWorkImagesResponseBuilder b)]) = _$AttachWorkImagesResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(AttachWorkImagesResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<AttachWorkImagesResponse> get serializer => _$AttachWorkImagesResponseSerializer();
}

class _$AttachWorkImagesResponseSerializer implements PrimitiveSerializer<AttachWorkImagesResponse> {
  @override
  final Iterable<Type> types = const [AttachWorkImagesResponse, _$AttachWorkImagesResponse];

  @override
  final String wireName = r'AttachWorkImagesResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    AttachWorkImagesResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.items != null) {
      yield r'items';
      yield serializers.serialize(
        object.items,
        specifiedType: const FullType.nullable(BuiltList, [FullType(OpportunityWorkImageResponse)]),
      );
    }
    if (object.opportunityRowVersion != null) {
      yield r'opportunityRowVersion';
      yield serializers.serialize(
        object.opportunityRowVersion,
        specifiedType: const FullType(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    AttachWorkImagesResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required AttachWorkImagesResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'items':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(OpportunityWorkImageResponse)]),
          ) as BuiltList<OpportunityWorkImageResponse>?;
          if (valueDes == null) continue;
          result.items.replace(valueDes);
          break;
        case r'opportunityRowVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.opportunityRowVersion = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  AttachWorkImagesResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = AttachWorkImagesResponseBuilder();
    final serializedList = (serialized as Iterable<Object?>).toList();
    final unhandled = <Object?>[];
    _deserializeProperties(
      serializers,
      serialized,
      specifiedType: specifiedType,
      serializedList: serializedList,
      unhandled: unhandled,
      result: result,
    );
    return result.build();
  }
}


