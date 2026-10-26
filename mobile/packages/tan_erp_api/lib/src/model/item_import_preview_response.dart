//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/item_import_row_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'item_import_preview_response.g.dart';

/// ItemImportPreviewResponse
///
/// Properties:
/// * [contentSha256] 
/// * [totalRows] 
/// * [validRows] 
/// * [invalidRows] 
/// * [rows] 
/// * [createCount] 
/// * [updateCount] 
/// * [unchangedCount] 
@BuiltValue()
abstract class ItemImportPreviewResponse implements Built<ItemImportPreviewResponse, ItemImportPreviewResponseBuilder> {
  @BuiltValueField(wireName: r'contentSha256')
  String? get contentSha256;

  @BuiltValueField(wireName: r'totalRows')
  int? get totalRows;

  @BuiltValueField(wireName: r'validRows')
  int? get validRows;

  @BuiltValueField(wireName: r'invalidRows')
  int? get invalidRows;

  @BuiltValueField(wireName: r'rows')
  BuiltList<ItemImportRowResponse>? get rows;

  @BuiltValueField(wireName: r'createCount')
  int? get createCount;

  @BuiltValueField(wireName: r'updateCount')
  int? get updateCount;

  @BuiltValueField(wireName: r'unchangedCount')
  int? get unchangedCount;

  ItemImportPreviewResponse._();

  factory ItemImportPreviewResponse([void updates(ItemImportPreviewResponseBuilder b)]) = _$ItemImportPreviewResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ItemImportPreviewResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ItemImportPreviewResponse> get serializer => _$ItemImportPreviewResponseSerializer();
}

class _$ItemImportPreviewResponseSerializer implements PrimitiveSerializer<ItemImportPreviewResponse> {
  @override
  final Iterable<Type> types = const [ItemImportPreviewResponse, _$ItemImportPreviewResponse];

  @override
  final String wireName = r'ItemImportPreviewResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ItemImportPreviewResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.contentSha256 != null) {
      yield r'contentSha256';
      yield serializers.serialize(
        object.contentSha256,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.totalRows != null) {
      yield r'totalRows';
      yield serializers.serialize(
        object.totalRows,
        specifiedType: const FullType(int),
      );
    }
    if (object.validRows != null) {
      yield r'validRows';
      yield serializers.serialize(
        object.validRows,
        specifiedType: const FullType(int),
      );
    }
    if (object.invalidRows != null) {
      yield r'invalidRows';
      yield serializers.serialize(
        object.invalidRows,
        specifiedType: const FullType(int),
      );
    }
    if (object.rows != null) {
      yield r'rows';
      yield serializers.serialize(
        object.rows,
        specifiedType: const FullType.nullable(BuiltList, [FullType(ItemImportRowResponse)]),
      );
    }
    if (object.createCount != null) {
      yield r'createCount';
      yield serializers.serialize(
        object.createCount,
        specifiedType: const FullType(int),
      );
    }
    if (object.updateCount != null) {
      yield r'updateCount';
      yield serializers.serialize(
        object.updateCount,
        specifiedType: const FullType(int),
      );
    }
    if (object.unchangedCount != null) {
      yield r'unchangedCount';
      yield serializers.serialize(
        object.unchangedCount,
        specifiedType: const FullType(int),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    ItemImportPreviewResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ItemImportPreviewResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'contentSha256':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.contentSha256 = valueDes;
          break;
        case r'totalRows':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.totalRows = valueDes;
          break;
        case r'validRows':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.validRows = valueDes;
          break;
        case r'invalidRows':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.invalidRows = valueDes;
          break;
        case r'rows':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(ItemImportRowResponse)]),
          ) as BuiltList<ItemImportRowResponse>?;
          if (valueDes == null) continue;
          result.rows.replace(valueDes);
          break;
        case r'createCount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.createCount = valueDes;
          break;
        case r'updateCount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.updateCount = valueDes;
          break;
        case r'unchangedCount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.unchangedCount = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  ItemImportPreviewResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ItemImportPreviewResponseBuilder();
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


