//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'item_import_commit_response.g.dart';

/// ItemImportCommitResponse
///
/// Properties:
/// * [batchId] 
/// * [createdCount] 
/// * [contentSha256] 
/// * [replayed] 
/// * [updatedCount] 
/// * [unchangedCount] 
@BuiltValue()
abstract class ItemImportCommitResponse implements Built<ItemImportCommitResponse, ItemImportCommitResponseBuilder> {
  @BuiltValueField(wireName: r'batchId')
  String? get batchId;

  @BuiltValueField(wireName: r'createdCount')
  int? get createdCount;

  @BuiltValueField(wireName: r'contentSha256')
  String? get contentSha256;

  @BuiltValueField(wireName: r'replayed')
  bool? get replayed;

  @BuiltValueField(wireName: r'updatedCount')
  int? get updatedCount;

  @BuiltValueField(wireName: r'unchangedCount')
  int? get unchangedCount;

  ItemImportCommitResponse._();

  factory ItemImportCommitResponse([void updates(ItemImportCommitResponseBuilder b)]) = _$ItemImportCommitResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ItemImportCommitResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ItemImportCommitResponse> get serializer => _$ItemImportCommitResponseSerializer();
}

class _$ItemImportCommitResponseSerializer implements PrimitiveSerializer<ItemImportCommitResponse> {
  @override
  final Iterable<Type> types = const [ItemImportCommitResponse, _$ItemImportCommitResponse];

  @override
  final String wireName = r'ItemImportCommitResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ItemImportCommitResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.batchId != null) {
      yield r'batchId';
      yield serializers.serialize(
        object.batchId,
        specifiedType: const FullType(String),
      );
    }
    if (object.createdCount != null) {
      yield r'createdCount';
      yield serializers.serialize(
        object.createdCount,
        specifiedType: const FullType(int),
      );
    }
    if (object.contentSha256 != null) {
      yield r'contentSha256';
      yield serializers.serialize(
        object.contentSha256,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.replayed != null) {
      yield r'replayed';
      yield serializers.serialize(
        object.replayed,
        specifiedType: const FullType(bool),
      );
    }
    if (object.updatedCount != null) {
      yield r'updatedCount';
      yield serializers.serialize(
        object.updatedCount,
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
    ItemImportCommitResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ItemImportCommitResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'batchId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.batchId = valueDes;
          break;
        case r'createdCount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.createdCount = valueDes;
          break;
        case r'contentSha256':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.contentSha256 = valueDes;
          break;
        case r'replayed':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.replayed = valueDes;
          break;
        case r'updatedCount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.updatedCount = valueDes;
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
  ItemImportCommitResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ItemImportCommitResponseBuilder();
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


