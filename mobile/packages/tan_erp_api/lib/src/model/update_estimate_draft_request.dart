//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/update_estimate_section_dto.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'update_estimate_draft_request.g.dart';

/// UpdateEstimateDraftRequest
///
/// Properties:
/// * [expectedRevisionVersion] 
/// * [sections] 
@BuiltValue()
abstract class UpdateEstimateDraftRequest implements Built<UpdateEstimateDraftRequest, UpdateEstimateDraftRequestBuilder> {
  @BuiltValueField(wireName: r'expectedRevisionVersion')
  String? get expectedRevisionVersion;

  @BuiltValueField(wireName: r'sections')
  BuiltList<UpdateEstimateSectionDto>? get sections;

  UpdateEstimateDraftRequest._();

  factory UpdateEstimateDraftRequest([void updates(UpdateEstimateDraftRequestBuilder b)]) = _$UpdateEstimateDraftRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(UpdateEstimateDraftRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<UpdateEstimateDraftRequest> get serializer => _$UpdateEstimateDraftRequestSerializer();
}

class _$UpdateEstimateDraftRequestSerializer implements PrimitiveSerializer<UpdateEstimateDraftRequest> {
  @override
  final Iterable<Type> types = const [UpdateEstimateDraftRequest, _$UpdateEstimateDraftRequest];

  @override
  final String wireName = r'UpdateEstimateDraftRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    UpdateEstimateDraftRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.expectedRevisionVersion != null) {
      yield r'expectedRevisionVersion';
      yield serializers.serialize(
        object.expectedRevisionVersion,
        specifiedType: const FullType(String),
      );
    }
    if (object.sections != null) {
      yield r'sections';
      yield serializers.serialize(
        object.sections,
        specifiedType: const FullType.nullable(BuiltList, [FullType(UpdateEstimateSectionDto)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    UpdateEstimateDraftRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required UpdateEstimateDraftRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'expectedRevisionVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.expectedRevisionVersion = valueDes;
          break;
        case r'sections':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(UpdateEstimateSectionDto)]),
          ) as BuiltList<UpdateEstimateSectionDto>?;
          if (valueDes == null) continue;
          result.sections.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  UpdateEstimateDraftRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = UpdateEstimateDraftRequestBuilder();
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


