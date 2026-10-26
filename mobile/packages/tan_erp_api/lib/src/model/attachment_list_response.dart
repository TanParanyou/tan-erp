//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/attachment_link_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'attachment_list_response.g.dart';

/// AttachmentListResponse
///
/// Properties:
/// * [items] 
@BuiltValue()
abstract class AttachmentListResponse implements Built<AttachmentListResponse, AttachmentListResponseBuilder> {
  @BuiltValueField(wireName: r'items')
  BuiltList<AttachmentLinkResponse>? get items;

  AttachmentListResponse._();

  factory AttachmentListResponse([void updates(AttachmentListResponseBuilder b)]) = _$AttachmentListResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(AttachmentListResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<AttachmentListResponse> get serializer => _$AttachmentListResponseSerializer();
}

class _$AttachmentListResponseSerializer implements PrimitiveSerializer<AttachmentListResponse> {
  @override
  final Iterable<Type> types = const [AttachmentListResponse, _$AttachmentListResponse];

  @override
  final String wireName = r'AttachmentListResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    AttachmentListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.items != null) {
      yield r'items';
      yield serializers.serialize(
        object.items,
        specifiedType: const FullType.nullable(BuiltList, [FullType(AttachmentLinkResponse)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    AttachmentListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required AttachmentListResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'items':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(AttachmentLinkResponse)]),
          ) as BuiltList<AttachmentLinkResponse>?;
          if (valueDes == null) continue;
          result.items.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  AttachmentListResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = AttachmentListResponseBuilder();
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


