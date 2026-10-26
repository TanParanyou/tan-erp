//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/localized_text_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'item_image_summary_response.g.dart';

/// ItemImageSummaryResponse
///
/// Properties:
/// * [id] 
/// * [fileId] 
/// * [role] 
/// * [isPrimary] 
/// * [displayOrder] 
/// * [altText] 
/// * [caption] 
@BuiltValue()
abstract class ItemImageSummaryResponse implements Built<ItemImageSummaryResponse, ItemImageSummaryResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'fileId')
  String? get fileId;

  @BuiltValueField(wireName: r'role')
  String? get role;

  @BuiltValueField(wireName: r'isPrimary')
  bool? get isPrimary;

  @BuiltValueField(wireName: r'displayOrder')
  int? get displayOrder;

  @BuiltValueField(wireName: r'altText')
  LocalizedTextResponse? get altText;

  @BuiltValueField(wireName: r'caption')
  LocalizedTextResponse? get caption;

  ItemImageSummaryResponse._();

  factory ItemImageSummaryResponse([void updates(ItemImageSummaryResponseBuilder b)]) = _$ItemImageSummaryResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ItemImageSummaryResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ItemImageSummaryResponse> get serializer => _$ItemImageSummaryResponseSerializer();
}

class _$ItemImageSummaryResponseSerializer implements PrimitiveSerializer<ItemImageSummaryResponse> {
  @override
  final Iterable<Type> types = const [ItemImageSummaryResponse, _$ItemImageSummaryResponse];

  @override
  final String wireName = r'ItemImageSummaryResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ItemImageSummaryResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.fileId != null) {
      yield r'fileId';
      yield serializers.serialize(
        object.fileId,
        specifiedType: const FullType(String),
      );
    }
    if (object.role != null) {
      yield r'role';
      yield serializers.serialize(
        object.role,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.isPrimary != null) {
      yield r'isPrimary';
      yield serializers.serialize(
        object.isPrimary,
        specifiedType: const FullType(bool),
      );
    }
    if (object.displayOrder != null) {
      yield r'displayOrder';
      yield serializers.serialize(
        object.displayOrder,
        specifiedType: const FullType(int),
      );
    }
    if (object.altText != null) {
      yield r'altText';
      yield serializers.serialize(
        object.altText,
        specifiedType: const FullType(LocalizedTextResponse),
      );
    }
    if (object.caption != null) {
      yield r'caption';
      yield serializers.serialize(
        object.caption,
        specifiedType: const FullType(LocalizedTextResponse),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    ItemImageSummaryResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ItemImageSummaryResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'id':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.id = valueDes;
          break;
        case r'fileId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.fileId = valueDes;
          break;
        case r'role':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.role = valueDes;
          break;
        case r'isPrimary':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.isPrimary = valueDes;
          break;
        case r'displayOrder':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.displayOrder = valueDes;
          break;
        case r'altText':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(LocalizedTextResponse),
          ) as LocalizedTextResponse?;
          if (valueDes == null) continue;
          result.altText.replace(valueDes);
          break;
        case r'caption':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(LocalizedTextResponse),
          ) as LocalizedTextResponse?;
          if (valueDes == null) continue;
          result.caption.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  ItemImageSummaryResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ItemImageSummaryResponseBuilder();
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


